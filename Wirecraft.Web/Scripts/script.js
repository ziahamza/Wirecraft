"strict"
var utils = {
	orderStatusIndex: ["completed", "stopped", "pending"],
	blobTypeIndex: ["image", "document", "video", "other"],
	getObservableObject: function (obj) {
	    var observableEntity = {};
	    for (var entity in obj) {
	        if (Object.prototype.toString.call(obj[entity]) === '[object Array]') {
	            observableEntity[entity] = ko.observableArray(obj[entity].map(ko.observable));
	        }
	        else {
	            observableEntity[entity] = ko.observable(obj[entity]);
	        }
	    }
	    return observableEntity;
	},
	convertToViewModel: function (data) {
		var that = this;
        var viewModel = {};
        for (var table in data) {
            var observableTable = ko.observableArray(data[table].map(that.getObservableObject));

            viewModel[table] = observableTable;
        }
        viewModel.getCustomerById = _.bind(function (id) {
        	var customer = null;
        	_.each(this.customers(), function (e) {
        		if (e.customerID() === id) {
        			customer = e;
        		}
        	});
        	return customer;
        }, (viewModel));

        viewModel.getCustomerOrders = _.bind(function (id) {
            return _.chain(this.orders())
                .filter(function (e) {
                    return e.customerID() === id;
                }).value();
        }, (viewModel));

        viewModel.getProductById = _.bind(function (id) {
        	var product = null;
        	_.each(this.products(), function (e) {
        		if (e.productID() === id) {
        			product = e;
        		}
        	});
        	return product;
        }, viewModel);

        viewModel.getBlobById = _.bind(function (id) {
            var blob = null;
            _.each(this.blobs(), function (e) {
                if (e.blobID() === id) {
                    blob = e;
                }
            });
            return blob;
        }, viewModel);

        viewModel.getOrdersByStatus = _.bind(function (status) {
        	return _.filter(this.orders(), function (e) {
        		return e.status() === utils.orderStatusIndex.indexOf(status);
			});
        }, viewModel);

        viewModel.getOrderPrice = _.bind(function (order) {
        	var that = this;
        	var price = 0;
        	_.chain(order.productIDs())
			.map(function (e) {
				price += that.getProductById(e()).price();
			})
        	price -= order.discount();
        	return price;
        }, viewModel);

        viewModel.getProductImage = _.bind(function (id) {
        	if (this.getPoductById(id).fileIDs().length) {
        		return "blob/imageById/" + _.chain(this.getProductById(id).fileIDs())
                .map(function (e) { return e(); })
                .map(this.getBlobById)
                .filter(function (e) {
                	return utils.blobTypeIndex[e.type()] === "image";
                }).first().value().blobID();
        	}
        	else {
        		console.log("images not found!!");
        		return "error.html";
        	}
        }, viewModel);

        viewModel.getProductImages = _.bind(function (id) {
        	return _.chain(this.getProductById(id).fileIDs())
                .map(function (e) { return e(); })
                .map(this.getBlobById)
                .filter(function (e) {
                	return utils.blobTypeIndex[e.type()] === "image";
                }).map(function (e) {
                	return {
                		href: "blob/imageById/" + e.blobID() + "#" + e.name(),
                		blobID: e.blobID()
                	}
                }).value();
        }, viewModel);

        viewModel.getProductOrders = _.bind(function (id) {
        	return _.chain(this.orders())
                .filter(function (e) {
                	return _.filter(e.productIDs(), function (e) { return e() === id; }).length > 0;
                }).value();
        }, (viewModel));

        viewModel.deleteBlob = _.bind(function (blobID) {
        	$.ajax({
        		url: "blob/delete/" + blobID
        	});
        }, (viewModel));
        return viewModel;
    }
};

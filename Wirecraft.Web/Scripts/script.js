"strict"
var utils = {
	orderStatusIndex: ["completed", "stopped", "pending"],
	blobTypeIndex: ["image", "document", "video", "other"],
    convertToViewModel: function(data) {
        var viewModel = {};
        for (var table in data) {
            var observableTable = ko.observableArray(data[table].map(function (e) {
                var observableEntity = {};
                for (var entity in e) {
                    if (Object.prototype.toString.call(e[entity]) === '[object Array]') {
                        observableEntity[entity] = ko.observableArray(e[entity].map(ko.observable));
                    }
                    else {
                        observableEntity[entity] = ko.observable(e[entity]);
                    }
                }
                return observableEntity;
            }));

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

        viewModel.getProductById = _.bind(function (id) {
        	var product = null;
        	_.each(this.products(), function (e) {
        		if (e.productID() === id) {
        			product = e;
        		}
        	});
        	return product;
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
        return viewModel;
    }
};

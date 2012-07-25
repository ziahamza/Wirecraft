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
	        else if (obj[entity] === null) {
	        	observableEntity[entity] = ko.observableArray([]);
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

        viewModel.getOrderById = _.bind(function (id) {
            var order = null;
            _.each(this.orders(), function (e) {
                if (e.orderID() === id) {
                    order = e;
                }
            });
            return order;
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
        	for (var i = 0; i < order.productIDs().length; i++) {
        	    price += that.getProductById(order.productIDs()[i]()).price() * order.quantities()[i]();
        	}

        	price -= order.discount();
        	return price;
        }, viewModel);

        viewModel.getProductImage = _.bind(function (id) {
            var image = _.chain(this.getProductById(id).fileIDs())
            .map(function (e) { return e(); })
            .map(this.getBlobById)
            .filter(function (e) {
                return utils.blobTypeIndex[e.type()] === "image";
            }).first().value();
            if (image) {
                return "blob/imageById/" + image.blobID();
            }
            else {
                console.log("images not found!!");
                return "error.html";
            }
        	
        }, viewModel);

        viewModel.getProductImages = _.bind(function (id) {
        	var fileIDs = this.getProductById(id).fileIDs();
        	if (fileIDs.length) {
        		return _.chain(fileIDs)
        		.map(function (e) { return e(); })
                .map(this.getBlobById)
                .filter(function (e) {
                	return utils.blobTypeIndex[e.type()] === "image";
                }).value();
        	} else {
        		return [];
        	}
        }, viewModel);

        viewModel.getProductNonImages = _.bind(function (id) {
            return _.chain(this.getProductById(id).fileIDs())
                .map(function (e) { return e(); })
                .map(this.getBlobById)
                .filter(function (e) {
                    return utils.blobTypeIndex[e.type()] != "image";
                }).value();
        }, viewModel);

        viewModel.getProductOrders = _.bind(function (id) {
        	return _.chain(this.orders())
                .filter(function (e) {
                	return _.filter(e.productIDs(), function (e) { return e() === id; }).length > 0;
                }).value();
        }, (viewModel));

        viewModel.deleteBlob = _.bind(function (blob) {
        	$.ajax({
        	    url: "blob/delete/" + blob.blobID(),
                type: "post",
                success: function (e) { console.log(e); },
                error: function (e) { console.log(e) }
        	});
        }, viewModel);

        viewModel.saveProduct = _.bind(function (oldProd) {
            var product = {
                name: $("#productName").val(),
                price: $("#productPrice").val(),
                description: $("#productDescription").val(),
                productID: oldProd.productID()
            };
            console.log(product);
            $.ajax({
                url: "product/update",
                data: product,
                type: "post",
                success: function () {
                    document.location.href = '#/product/id/' + oldProd.productID();
                },
                error: function () {
                    alert("some error occured processing the request!!");
                    document.location.href = '#/product/id/' + oldProd.productID();
                }
            });
        }, viewModel);

        viewModel.saveOrder = _.bind(function (oldOrder) {
            var order = {
                customerID: $("#orderCustomer").val(),
                discount: $("#orderDiscount").val(),
                address: $("#orderAddress").val(),
                orderID: oldOrder.orderID(),
                status: $("#orderStatus").val()
            };
            console.log(order);
            $.ajax({
                url: "order/update",
                data: order,
                type: "post",
                success: function () {
                    document.location.href = '#/order/id/' + oldOrder.orderID();
                },
                error: function () {
                    alert("some error occured processing the request!!");
                    document.location.href = '#/order/id/' + oldOrder.orderID();
                }
            });
        }, viewModel);

        viewModel.getOrderProducts = _.bind(function (order) {
            return _.filter(viewModel.products(), function (prod) {
                return _.map(order.productIDs(), function (e) { return e(); }).indexOf(prod.productID()) != -1;
            });
        }, viewModel);

        viewModel.updateOrderProduct = _.bind(function (productID, orderID, quantity) {
            $.ajax({
                url: 'order/updateProduct/' + orderID,
                type: 'post',
                data: {
                    productID: productID,
                    quantity: quantity
                },
                success: function () { },
                error: function (ex) { console.log(ex); }
            });
        }, viewModel);

        viewModel.getOrderStatusHTML = _.bind(function (status) {
            var html = "";
            for (var i = 0; i < utils.orderStatusIndex.length; i++) {
                html += '<option value="' + i + '" ';
                if (status == i) {
                    console.log("selected:" + status);
                    html += 'selected="true"';
                }
                html += ">" + utils.orderStatusIndex[i] + "</option>";
            }
            return html;
        }, viewModel);

        viewModel.getOrderCustomersHTML = _.bind(function (customerID) {
            var html = "";
            _.each(this.customers(), function (c) {
                html += '<option value="' + c.customerID() + '" ';
                if (c.customerID() == customerID) {
                    console.log("selected:" + customerID);
                    html += 'selected="true"';
                }
                html += ">" + c.name() + "</option>";
            });
            return html;
        }, viewModel);

        viewModel.getFileType = _.bind(function (type) {
            return [
                'Image',
                'Doc',
                'Video',
                'Other'
            ][type];
        }, viewModel);

        viewModel.addOrder = _.bind(function () {
        	$.ajax({
        		url: 'order/add',
        		type: 'post',
        		success: function (data) {
        			var id = data.orderID;
        			document.location.href = '#/order/edit/' + id;
        		},
        		error: function (err) {
        			console.log(err);
        		}
        	});
        });

        viewModel.addCustomer = _.bind(function () {
        	$.ajax({
        		url: 'customer/add',
        		type: 'post',
        		success: function (data) {
        			var id = data.customerID;
        			document.location.href = '#/customer/edit/' + id;
        		},
        		error: function (err) {
        			console.log(err);
        		}
        	});
        });


        viewModel.deleteOrder = _.bind(function (order) {
        	$.ajax({
        		url: 'order/delete/' + order.orderID(),
        		type: 'post',
        		success: function () {
        			document.location.href = '#/orders/status/pending';
        		},
        		error: function (err) {
        			console.log(err);
        			document.location.href = '#/orders/status/pending';
        		}
        	});
        });

        viewModel.deleteProduct = _.bind(function (product) {
        	$.ajax({
        		url: 'product/delete/' + product.productID(),
        		type: 'post',
        		success: function () {
        			document.location.href = '#/products/overview';
        		},
        		error: function (err) {
        			console.log(err);
        			document.location.href = '#/products/overview';
        		}
        	});
        });

        viewModel.deleteCustomer = _.bind(function (id) {
        	console.log(id);
        	$.ajax({
        		url: 'customer/delete/' + id,
        		type: 'post',
        		success: function () {
        			document.location.href = '#/customers/overview';
        		},
        		error: function (err) {
        			console.log(err);
        			document.location.href = '#/customers/overview';
        		}
        	});
        });

        viewModel.addProduct = _.bind(function () {
        	$.ajax({
        		url: 'product/add',
        		type: 'post',
        		success: function (data) {
        			document.location.href = '#/product/edit/' + data.productID;
        		},
        		error: function (err) {
        			console.log(err);
        			alert("Error occured!!");
        		}
        	});
        });
        return viewModel;
    }
};

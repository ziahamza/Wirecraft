var viewModel = utils.convertToViewModel(objGraph);
var dataAccessHub = $.connection.dataAccessHub;

(function () {
	dataAccessHub.updateModel = function (changes) {
		console.log("updating changes!!!");
		utils.trace(changes);
		_.each(changes, function (e) {
			var data = e.data;
			if (e.op === "add") {
				(function (tables) {
					_.each(tables, function (table) {
						_.each(data[table], function (e) {
							console.log("adding:" + JSON.stringify(e) + " to:" + table);
							viewModel[table].push(utils.getObservableObject(e));
						});
					});
				})(["blobs", "customers", "orders", "products"]);
			}
			if (e.op === "delete") {
				(function (tables) {
					_.each(tables, function (table) {
						_.each(data[table], function (e) {
							viewModel[table].remove(function (k) {
								return k[table.slice(0, -1) + "ID"]() === e[table.slice(0, -1) + "ID"];
							});
						});
					});
				})(["blobs", "customers", "orders", "products"]);
			}
			if (e.op === "update") {
				(function (tables) {
					_.each(tables, function (table) {
						_.each(data[table], function (e) {
							var entity = _.chain(viewModel[table]()).filter(function (k) {
								return k[table.slice(0, -1) + "ID"]() === e[table.slice(0, -1) + "ID"];
							}).last().value();
							for (var key in e) {
								if (Object.prototype.toString.call(e[key]) === '[object Array]') {
									if (!entity[key]) {
										entity[key] = ko.observableArray(e[key]);
									} else {
										for (var i = 0; i < e[key].length; i++) {
											if (entity[key]() && entity[key]()[i]) {
												entity[key]()[i](e[key][i]);
											}
											else {
												entity[key].push(ko.observable(e[key][i]));
											}
										}
										if (entity[key]().length > e[key].length) {
											for (var i = e[key].length; i < entity[key]().length; i++) {
												entity[key].pop();
											}
										}
									}
								}
								else {
									if (!entity[key]) {
										entity[key] = ko.observable();
									}
									entity[key](e[key]);
								}
							};
						});
					});
				})(["blobs", "customers", "orders", "products"]);
			}
		});
	}
})();
(function () {
	crossroads.addRoute("orders/status/{status}", function (status) {
		$(".nav-collapse .nav li").removeClass("active");
		$('.nav li a[href="#/orders/status/pending"]').parent().addClass("active");
		var mainBody = $("#mainBody");
		if (utils.orderStatusIndex.indexOf(status) === -1)
			return;
		var html = $("#ordersStatusView").html();
		var parsed = _.template(html, {
			ordersFunc: "$root.getOrdersByStatus('" + status + "')",
			status: status
		}, { variable: 'data' });
		mainBody.html(parsed);
		ko.applyBindings(viewModel);
	});

	crossroads.addRoute("order/id/{id}", function (id) {
		$(".nav-collapse .nav li").removeClass("active");
		$('.nav li a[href="#/orders/status/pending"]').parent().addClass("active");
		var mainBody = $("#mainBody");
		var html = $("#orderDetailView").html();
		var parsed = _.template(html, {
			orderID: id
		}, { variable: "data" });
		mainBody.html(parsed);
		ko.applyBindings(viewModel, mainBody[0]);
	});

	crossroads.addRoute("products/overview", function () {
		$(".nav-collapse .nav li").removeClass("active");
		$('.nav li a[href="#/products/overview"]').parent().addClass("active");
		var mainBody = $("#mainBody");
		var html = $("#productsOverviewView").html();
		var parsed = _.template(html, {
			productsFunc: "products"
		}, { variable: "data" });
		mainBody.html(parsed);
		ko.applyBindings(viewModel, mainBody[0]);
	});
	crossroads.addRoute("product/id/{id}", function (id) {
		$(".nav-collapse .nav li").removeClass("active");
		$('.nav li a[href="#/products/overview"]').parent().addClass("active");
		var mainBody = $("#mainBody");
		var html = $("#productDetailView").html();
		var parsed = _.template(html, {
			productID: id
		}, { variable: "data" });
		mainBody.html(parsed);
		ko.applyBindings(viewModel, mainBody[0]);
	});

	crossroads.addRoute("customers/overview", function () {
		$(".nav-collapse .nav li").removeClass("active");
		$('.nav li a[href="#/customers/overview"]').parent().addClass("active");
		var mainBody = $("#mainBody");
		var html = $("#customersOverviewView").html();
		var parsed = _.template(html, {
			customersFunc: "customers"
		}, { variable: "data" });
		mainBody.html(parsed);
		ko.applyBindings(viewModel, mainBody[0]);
	});

	crossroads.addRoute("customer/id/{id}", function (id) {
		$(".nav-collapse .nav li").removeClass("active");
		$('.nav li a[href="#/customers/overview"]').parent().addClass("active");
		var mainBody = $("#mainBody");
		var html = $("#customerDetailView").html();
		var parsed = _.template(html, {
			customerID: id
		}, { variable: "data" });
		mainBody.html(parsed);
		ko.applyBindings(viewModel, mainBody[0]);
	});

	crossroads.addRoute("customer/edit/{id}", function (id) {
		$(".nav-collapse .nav li").removeClass("active");
		$('.nav li a[href="#/customers/overview"]').parent().addClass("active");
		var mainBody = $("#mainBody");
		var html = $("#customerEditView").html();
		var parsed = _.template(html, {
			customerID: id
		}, { variable: "data" });
		mainBody.html(parsed);
		ko.applyBindings(viewModel, mainBody[0]);

		$('.datePicker').datepicker();

		var uploader = new qq.FileUploader({
			element: document.getElementById('customerPhoto'),
			action: '/customer/uploadPhoto/' + id,
			debug: true
		});

		$("#saveCustomer").click(function () {
			var customer = {
				birthDay: (new Date($('#customerBirthDay').val())).toJSON(),
				name: $('#customerName').val(),
				customerID: id
			};
			console.log(customer);
			$.ajax({
				url: "customer/update",
				data: customer,
				type: "post",
				success: function () {
					document.location.href = '#/customer/id/' + id;
				},
				error: function () {
					alert("some error occured processing the request!!");
					document.location.href = '#/customer/id/' + id;
				}
			});

			return false;
		});
	});

	crossroads.addRoute("product/edit/{id}", function (id) {
		$(".nav-collapse .nav li").removeClass("active");
		$('.nav li a[href="#/products/overview"]').parent().addClass("active");
		var mainBody = $("#mainBody");
		var html = $("#productEditView").html();
		var parsed = _.template(html, {
			productID: id
		}, { variable: "data" });
		mainBody.html(parsed);
		ko.applyBindings(viewModel, mainBody[0]);


		var uploader = new qq.FileUploader({
			element: document.getElementById('productFile'),
			action: '/product/uploadFile/' + id,
			params: {
				type: $("#productFileType").val()
			},
			debug: true
		});
		$("#productFileType").change(function () {
			uploader.setParams({
				type: $("#productFileType").val()
			});
		});
	});

	crossroads.addRoute("order/edit/{id}", function (id) {
		$(".nav-collapse .nav li").removeClass("active");
		$('.nav li a[href="#/orders/status/pending"]').parent().addClass("active");
		var mainBody = $("#mainBody");
		var html = $("#orderEditView").html();
		var parsed = _.template(html, {
			orderID: id
		}, { variable: "data" });
		mainBody.html(parsed);
		ko.applyBindings(viewModel, mainBody[0]);


		/*$("#productFileType").change(function () {
			uploader.setParams({
				type: $("#productFileType").val()
			});
		});*/
	});
})();
$.connection.hub.start(function () {
	(function () {

		function parseHash(newHash, oldHash) {
			crossroads.parse(newHash);
		}
		hasher.initialized.add(parseHash); //parse initial hash
		hasher.changed.add(parseHash); //parse hash changes
		hasher.init(); //start listening for history change

		//update URL fragment generating new history record
		hasher.setHash('orders/status/pending');
	})();
});
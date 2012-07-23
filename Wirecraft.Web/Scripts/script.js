var utils = {
    convertToViewModel: function(data) {
        var viewModel = {};
        for (var table in data) {
            var observableTable = ko.observableArray(data[table].map(function (e) {
                var observableEntity = {};
                for (var entity in e) {
                    if (Object.prototype.toString.call(e[entity]) === '[object Array]') {
                        observableEntity[entity] = ko.observableArray([entity].map(ko.observable));
                    }
                    else {
                        observableEntity[entity] = ko.observable(e[entity]);
                    }
                }
                return observableEntity;
            }));

            viewModel[table] = observableTable;
        }
        return viewModel;
    }
};
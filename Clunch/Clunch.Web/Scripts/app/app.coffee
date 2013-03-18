'use strict'

angular.module('clunch', ['clunchServices'])
    .config(['$routeProvider', ($routeProvider) ->
        $routeProvider
            .when('/contacts',
                templateUrl: 'Templates/contactList.html'
                controller: 'ContactList')
            .when('/contacts/create',
                templateUrl: 'Templates/contactCreate.html'
                controller: 'ContactCreate')
            .when('/contacts/edit/:id',
                templateUrl: 'Templates/contactEdit.html'
                controller: 'ContactEdit')
            .otherwise
                redirectTo: '/contacts'
    ])
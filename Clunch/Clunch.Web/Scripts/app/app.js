﻿(function() {
  'use strict';

  angular.module('clunch', ['clunchServices']).config([
    '$routeProvider', function($routeProvider) {
      return $routeProvider.when('/contacts', {
        templateUrl: 'Templates/contactDetail.html',
        controller: 'ContactDetail'
      }).when('/contacts/create', {
        templateUrl: 'Templates/contactCreate.html',
        controller: 'ContactCreate'
      }).when('/contacts/edit/:id', {
        templateUrl: 'Templates/contactEdit.html',
        controller: 'ContactEdit'
      }).otherwise({
        redirectTo: '/contacts'
      });
    }
  ]);

}).call(this);

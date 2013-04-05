(function() {
  'use strict';

  var services;

  services = angular.module('clunchServices', ['ngResource']);

  services.factory('Contact', [
    '$resource', function($resource) {
      return $resource('api/contacts/:id', {
        id: '@Id'
      });
    }
  ]);

}).call(this);

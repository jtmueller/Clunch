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

  services.factory('ClunchHub', function() {
    var clunchHub;
    clunchHub = $.connection.clunchHub;
    $.connection.hub.start();
    return clunchHub;
  });

}).call(this);

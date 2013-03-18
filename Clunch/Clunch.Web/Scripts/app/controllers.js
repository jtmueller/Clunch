(function() {
  'use strict';

  var app;

  app = angular.module('clunch');

  app.controller('ContactDetail', [
    '$scope', '$http', function($scope, $http) {
      $http.get('/api/contacts').success(function(data) {
        return $scope.contacts = data;
      });
      $scope.deleteContact = function(id) {
        $scope.contacts = _.filter($scope.contacts, function(c) {
          return c.Id !== id;
        });
        return $http["delete"]("/api/" + id, {
          headers: {
            'Content-Type': 'application/json'
          }
        }).success(function() {
          return toastr.success('You have successfully deleted your contact!', 'Success!');
        }).error(function() {
          console.log('Error: Delete Contact', arguments);
          return toastr.error('There was an error deleting your contact.', '<sad face>');
        });
      };
    }
  ]);

  app.controller('ContactCreate', [
    '$scope', '$http', function($scope, $http) {
      $scope.contact = {};
      $scope.saveContact = function() {
        return $http.post('/api/contacts', $scope.contact).success(function() {
          toastr.success('You have successfully saved your contact!', 'Success!');
          return window.location.href = '#/contacts';
        }).error(function() {
          console.log('Error: Create Contact', arguments);
          return toastr.error('There was an error saving your contact.', '<sad face>');
        });
      };
    }
  ]);

  app.controller('ContactEdit', [
    '$scope', '$routeParams', '$http', function($scope, $routeParams, $http) {
      $http.get("/api/contacts/" + $routeParams.id).success(function(data) {
        return $scope.contact = data;
      });
      $scope.saveContact = function() {
        return $http.post('/api/contacts', $scope.contact).success(function() {
          toastr.success('You have successfully saved your contact!', 'Success!');
          return window.location.href = '#/contacts';
        }).error(function() {
          console.log('Error: Create Contact', arguments);
          return toastr.error('There was an error saving your contact.', '<sad face>');
        });
      };
    }
  ]);

}).call(this);

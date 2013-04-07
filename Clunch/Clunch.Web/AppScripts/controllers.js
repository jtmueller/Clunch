(function() {
  'use strict';

  var app;

  app = angular.module('clunch');

  app.controller('ContactList', [
    '$scope', 'Contact', function($scope, Contact) {
      $scope.contacts = Contact.query();
      $scope.deleteContact = function(contact) {
        $scope.contacts.splice($scope.contacts.indexOf(contact), 1);
        return contact.$delete(function() {
          return toastr.success('You have successfully deleted your contact!', 'Success!');
        }, function() {
          console.log('Error: Delete Contact', arguments);
          return toastr.error('There was an error deleting your contact.', '<sad face>');
        });
      };
    }
  ]);

  app.controller('ContactCreate', [
    '$scope', 'Contact', function($scope, Contact) {
      $scope.contact = new Contact();
      $scope.title = "Create";
      $scope.saveContact = function() {
        return $scope.contact.$save(function() {
          toastr.success('You have successfully saved your contact!', 'Success!');
          return window.location.href = '#/contacts';
        }, function() {
          console.log('Error: Create Contact', arguments);
          return toastr.error('There was an error saving your contact.', '<sad face>');
        });
      };
    }
  ]);

  app.controller('ContactEdit', [
    '$scope', '$routeParams', 'Contact', function($scope, $routeParams, Contact) {
      $scope.contact = Contact.get($routeParams);
      $scope.title = "Edit";
      $scope.saveContact = function() {
        return $scope.contact.$save(function() {
          toastr.success('You have successfully saved your contact!', 'Success!');
          return window.location.href = '#/contacts';
        }, function() {
          console.log('Error: Edit Contact', arguments);
          return toastr.error('There was an error saving your contact.', '<sad face>');
        });
      };
    }
  ]);

}).call(this);

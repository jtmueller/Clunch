'use strict'

app = angular.module 'clunch'

app.controller 'ContactList', ['$scope', 'Contact', ($scope, Contact) ->
    $scope.contacts = Contact.query()

    $scope.deleteContact = (contact) ->
        $scope.contacts.splice $scope.contacts.indexOf(contact), 1
        contact.$delete(
            () -> toastr.success 'You have successfully deleted your contact!', 'Success!'
            ,
            () -> 
                console.log 'Error: Delete Contact', arguments
                toastr.error 'There was an error deleting your contact.', '<sad face>')
    return
]

app.controller 'ContactCreate', ['$scope', 'Contact', ($scope, Contact) ->

    $scope.contact = new Contact()

    $scope.saveContact = () ->
        $scope.contact.$save(
            () ->
                toastr.success 'You have successfully saved your contact!', 'Success!'
                window.location.href = '#/contacts'
            ,
            () ->
                console.log 'Error: Create Contact', arguments
                toastr.error 'There was an error saving your contact.', '<sad face>')

    return
]

app.controller 'ContactEdit', ['$scope', '$routeParams', 'Contact', ($scope, $routeParams, Contact) ->

    $scope.contact = Contact.get $routeParams

    $scope.saveContact = () ->
        $scope.contact.$save(
            () -> 
                toastr.success 'You have successfully saved your contact!', 'Success!'
                window.location.href = '#/contacts'
            ,
            () ->
                console.log 'Error: Edit Contact', arguments
                toastr.error 'There was an error saving your contact.', '<sad face>')

    return
]

'use strict'

app = angular.module 'clunch'

app.controller 'ContactDetail', ['$scope', '$http', ($scope, $http) ->
    $http.get('/api/contacts').success (data) ->
        $scope.contacts = data

    $scope.deleteContact = (id) ->
        $scope.contacts =
            _.filter $scope.contacts, (c) -> c.Id != id

        $http.delete("/api/#{id}",
            headers: { 'Content-Type': 'application/json' })
            .success(() -> toastr.success 'You have successfully deleted your contact!', 'Success!')
            .error(() -> 
                console.log 'Error: Delete Contact', arguments
                toastr.error 'There was an error deleting your contact.', '<sad face>')

    return
]

app.controller 'ContactCreate', ['$scope', '$http', ($scope, $http) ->

    $scope.contact = {}

    $scope.saveContact = () ->
        $http.post('/api/contacts', $scope.contact)
            .success(() -> 
                toastr.success 'You have successfully saved your contact!', 'Success!'
                window.location.href = '#/contacts')
            .error(() ->
                console.log 'Error: Create Contact', arguments
                toastr.error 'There was an error saving your contact.', '<sad face>')

    return
]

app.controller 'ContactEdit', ['$scope', '$routeParams', '$http', ($scope, $routeParams, $http) ->
    $http.get("/api/contacts/#{ $routeParams.id }").success (data) ->
        $scope.contact = data

    $scope.saveContact = () ->
        $http.post('/api/contacts', $scope.contact)
            .success(() -> 
                toastr.success 'You have successfully saved your contact!', 'Success!'
                window.location.href = '#/contacts')
            .error(() ->
                console.log 'Error: Create Contact', arguments
                toastr.error 'There was an error saving your contact.', '<sad face>')

    return
]
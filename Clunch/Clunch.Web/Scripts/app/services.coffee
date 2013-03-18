'use strict'

services = angular.module 'clunchServices', ['ngResource']

services.factory 'Contact', ['$resource', ($resource) ->
    $resource 'api/contacts/:id', { id:'@Id' }
]
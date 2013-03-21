'use strict'

services = angular.module 'clunchServices', ['ngResource']

services.factory 'Contact', ['$resource', ($resource) ->
    $resource 'api/contacts/:id', { id:'@Id' }
]

services.factory 'ClunchHub', () ->
    clunchHub = $.connection.clunchHub
    $.connection.hub.start()
    clunchHub


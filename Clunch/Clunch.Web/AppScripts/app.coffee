'use strict'

angular.module('clunch', ['clunchServices', 'ui.bootstrap', 'ui.bootstrap.dialog'])
    .config(['$routeProvider', '$dialogProvider', ($routeProvider, $dialogProvider) ->
        $routeProvider
            .when('/contacts',
                templateUrl: 'Templates/contactList.html'
                controller: 'ContactList')
            .when('/contacts/create',
                templateUrl: 'Templates/contactEdit.html'
                controller: 'ContactCreate')
            .when('/contacts/edit/:id',
                templateUrl: 'Templates/contactEdit.html'
                controller: 'ContactEdit')
            .when('/chat',
                template: '<console></console>')
            .otherwise
                redirectTo: '/contacts'

        $dialogProvider.options
            backdrop: true
            backdropFade: true

        return
    ])
    .run(['$rootScope', '$location', ($rootScope, $location) ->
        # register listener to watch route changes
        $rootScope.$on '$routeChangeSuccess', (event, next, current) ->
            path = '#' + $location.path()
            targetLink = $(".navbar a[href='#{ path }']")
            if targetLink.length > 0
                $('.navbar li.active').removeClass 'active'
                targetLink.closest('li').addClass 'active'
    ])
 
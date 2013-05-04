'use strict'

class Console
    constructor: ($scope, $dialog) ->
        @scope = $scope
        @scope.messages = []
        @el = $element
        @dialog = $dialog
        @outputScroll = @el.find '.console > div'
        @textBox = @el.find 'input[type="text"]'
        @el.find('form').submit @sendMessage
        @el.find('input[type="button"]').click @sendMessage

        @hub = $.connection.clunchHub

        @hub.client.addMessage = @addMessage
        @hub.client.login = @login
        @hub.client.log = (msg) -> console.log msg
        @hub.client.success = (msg, title) -> toastr.success msg, title
        @hub.client.info = (msg, title) -> toastr.info msg, title
        @hub.client.warning = (msg, title) -> toastr.warning msg, title
        @hub.client.error = (msg, title) -> 
            console.error msg
            toastr.error msg, title

        $.connection.hub.start()
            .fail (msg) -> 
                console.error msg
                toastr.error msg

    addMessage: (name, text, className) =>
        @scope.messages.push
            user: name
            text: text
            className: className
        if @scope.messages.length > 300
            @scope.messages.shift()
        @scope.$apply()
        @outputScroll.scrollTop(@outputScroll.prop 'scrollHeight')

    login: =>
        dlg = @dialog.dialog
            dialogClass: 'modal login'
            backdropClick: false
            keyboard: false
            template: 
                '''
                <div class="modal-header"><h4>Who are you?</h4></div>
                <div class="modal-body">
                    <form name="loginForm">
                        <input type="text" name="loginName" ng-model="name" required="required" size="30" maxlength="30" />
                    </form>
                </div>
                <div class="modal-footer">
                    <input type="button" class="btn" ng-click="login()" ng-disabled="loginForm.$invalid" value="Sign In" />
                </div>
                '''
            controller: ['$scope', 'dialog', ($scope, dialog) ->
                dialog.modalEl.find('form').submit -> dialog.close $scope.name
                $scope.login = -> dialog.close $scope.name
                setTimeout ->
                    dialog.modalEl.find('input[name="loginName"]').focus()
                , 250
            ]
        dlg.open().then (name) =>
            @hub.server.login name
            @textBox.focus()
        # This is needed to "pump the message loop" in angular so that 
        # promises resolve and the dialog displays
        @scope.$apply()

    sendMessage: (e) =>
        e.preventDefault()
        message = @textBox.val()
        return if message.length == 0
        try
            @hub.server.send message
        catch err
            console.error err
            toastr.error err.message
        @textBox.val ''
        @textBox.focus()


app = angular.module 'clunch'

app.controller 'Console', ['$scope', '$dialog', Console]

app.directive 'console', () ->
    restrict: 'E'
    replace: true
    scope: {}
    template: 
        '''
        <div>
            <div class="row-fluid">
                <div class="span5 console">
                    <div>
                        <div ng-repeat="message in messages" ng-class="message.className"><b>{{message.user}}:</b> {{message.text}}</div>
                    </div>
                </div>
            </div>
            <div class="row-fluid">
                <form name="consoleForm" class="navbar-form pull-left span5">
                    <input type="text" class="span10" required />
                    <input type="button" class="btn span2" value="Send" ng-disabled="consoleForm.$invalid" />
                </form>
            </div>
        </div>
        '''
    controller: ['$scope', '$element', '$dialog', Console]
    link: (scope, element, attrs) ->
        # The linking function adds behavior to the template



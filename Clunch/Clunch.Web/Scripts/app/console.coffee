'use strict'

class Console
    constructor: ($scope, $element, $dialog) ->
        @scope = $scope
        @el = $element
        @dialog = $dialog
        @output = @el.find '.console'
        @textBox = @el.find 'input[type="text"]'
        @el.find('form').submit @sendMessage
        @el.find('input[type="button"]').click @sendSMessage

        @hub = $.connection.clunchHub
        @hub.client.addMessage = @addMessage
        @hub.client.alertMessage = @alertMessage
        $.connection.hub.start()

    addMessage: (message) =>
        @output.append $('<div/>').text message
        @output.scrollTop(@output.prop 'scrollHeight')

    sendMessage: (e) =>
        e.preventDefault()
        message = @textBox.val()
        return if message.length == 0
        @hub.server.send message
        @textBox.val ''
        @textBox.focus()

    alertMessage: (title, message) ->
        #btns = [{result:'cancel', label: 'Cancel'}, {result:'ok', label: 'OK', cssClass: 'btn-primary'}];
        #@dialog.messageBox(title, message, btns).open()
        alert message


app = angular.module 'clunch'

app.directive 'console', () ->
    restrict: 'E'
    replace: true
    scope: {}
    template: 
        '''
        <div>
            <div class="row-fluid">
                <div class="span5 console">
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
    controller: Console
    link: (scope, element, attrs) ->
        # The linking function adds behavior to the template



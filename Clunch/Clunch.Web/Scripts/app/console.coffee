'use strict'

class Console
    constructor: ($scope, $element, $dialog) ->
        @scope = $scope
        @el = $element
        @dialog = $dialog
        @output = @el.find '.console'
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

    addMessage: (name, message, className) =>
        #TODO: consider using binding here?
        @output.append $('<div/>').addClass(className).text "#{name}: #{message}"
        @output.scrollTop(@output.prop 'scrollHeight')
        #msgbox = @dialog.messageBox title ? "Success", message, [{label:'Yes, I\'m sure', result: 'yes'},{label:'Nope', result: 'no'}]
        #msgbox.open().then (result) ->
        #    console.log(result)
        # this is needed to "pump the message loop" in angular so that promises resolve
        #@scope.$apply()

    login: () =>
        name = prompt 'Who are you?', ''
        if name.length > 0
            @hub.server.login name

    sendMessage: (e) =>
        e.preventDefault()
        message = @textBox.val()
        return if message.length == 0
        try
            @hub.server.send message
        catch err
            toastr.error err.message
        @textBox.val ''
        @textBox.focus()


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
    controller: ['$scope', '$element', '$dialog', Console]
    link: (scope, element, attrs) ->
        # The linking function adds behavior to the template



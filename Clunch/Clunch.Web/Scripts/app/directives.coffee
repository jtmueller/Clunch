'use strict'

app = angular.module 'clunch'

app.directive 'chat', ['ClunchHub', (ClunchHub) ->
    restrict: 'E'
    replace: true
    scope: {}
    template: 
        '''
        <div>
            <div class="row-fluid">
                <div class="span5 chatBox">
                </div>
            </div>
            <div class="row-fluid">
                <form name="chatForm" class="navbar-form pull-left span5">
                    <input type="text" class="span10" required />
                    <input type="button" class="btn span2" value="Send" ng-disabled="chatForm.$invalid" />
                </form>
            </div>
        </div>
        '''
    link: (scope, element, attrs) ->
        # The linking function adds behavior to the template
        chatBox = element.find '.chatBox'
        ClunchHub.client.addMessage = (message) ->
            chatBox.append $('<div/>').text message
            chatBox.scrollTop(chatBox.prop 'scrollHeight')

        textBox = element.find 'input[type="text"]'

        sendMessage = (e) ->
            e.preventDefault()
            message = textBox.val()
            return if message.length == 0
            ClunchHub.server.send message
            textBox.val ''
            textBox.focus()

        element.find('form').submit sendMessage
        element.find('input[type="button"]').click sendMessage

        return
]

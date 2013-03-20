(function() {
  'use strict';

  var app;

  app = angular.module('clunch');

  app.directive('chat', [
    'ClunchHub', function(ClunchHub) {
      return {
        restrict: 'E',
        replace: true,
        scope: {},
        template: '<div>\n    <div class="row-fluid">\n        <div class="span5 chatBox">\n        </div>\n    </div>\n    <div class="row-fluid">\n        <form name="chatForm" class="navbar-form pull-left span5">\n            <input type="text" class="span10" required />\n            <input type="button" class="btn span2" value="Send" ng-disabled="chatForm.$invalid" />\n        </form>\n    </div>\n</div>',
        link: function(scope, element, attrs) {
          var chatBox, sendMessage, textBox;
          chatBox = element.find('.chatBox');
          ClunchHub.client.addMessage = function(message) {
            chatBox.append($('<div/>').text(message));
            return chatBox.scrollTop(chatBox.prop('scrollHeight'));
          };
          textBox = element.find('input[type="text"]');
          sendMessage = function(e) {
            var message;
            e.preventDefault();
            message = textBox.val();
            if (message.length === 0) {
              return;
            }
            ClunchHub.server.send(message);
            textBox.val('');
            return textBox.focus();
          };
          element.find('form').submit(sendMessage);
          element.find('input[type="button"]').click(sendMessage);
        }
      };
    }
  ]);

}).call(this);

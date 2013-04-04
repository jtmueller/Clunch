(function() {
  'use strict';

  var Console, app,
    __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

  Console = (function() {

    function Console($scope, $element, $dialog) {
      this.sendMessage = __bind(this.sendMessage, this);

      this.addMessage = __bind(this.addMessage, this);
      this.scope = $scope;
      this.el = $element;
      this.dialog = $dialog;
      this.output = this.el.find('.console');
      this.textBox = this.el.find('input[type="text"]');
      this.el.find('form').submit(this.sendMessage);
      this.el.find('input[type="button"]').click(this.sendSMessage);
      this.hub = $.connection.clunchHub;
      this.hub.client.addMessage = this.addMessage;
      this.hub.client.alertMessage = this.alertMessage;
      $.connection.hub.start();
    }

    Console.prototype.addMessage = function(message) {
      this.output.append($('<div/>').text(message));
      return this.output.scrollTop(this.output.prop('scrollHeight'));
    };

    Console.prototype.sendMessage = function(e) {
      var message;
      e.preventDefault();
      message = this.textBox.val();
      if (message.length === 0) {
        return;
      }
      this.hub.server.send(message);
      this.textBox.val('');
      return this.textBox.focus();
    };

    Console.prototype.alertMessage = function(title, message) {
      return alert(message);
    };

    return Console;

  })();

  app = angular.module('clunch');

  app.directive('console', function() {
    return {
      restrict: 'E',
      replace: true,
      scope: {},
      template: '<div>\n    <div class="row-fluid">\n        <div class="span5 console">\n        </div>\n    </div>\n    <div class="row-fluid">\n        <form name="consoleForm" class="navbar-form pull-left span5">\n            <input type="text" class="span10" required />\n            <input type="button" class="btn span2" value="Send" ng-disabled="consoleForm.$invalid" />\n        </form>\n    </div>\n</div>',
      controller: Console,
      link: function(scope, element, attrs) {}
    };
  });

}).call(this);

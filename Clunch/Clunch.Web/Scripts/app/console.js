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
      this.el.find('input[type="button"]').click(this.sendMessage);
      this.hub = $.connection.clunchHub;
      this.hub.client.addMessage = this.addMessage;
      this.hub.client.log = function(msg) {
        return console.log(msg);
      };
      this.hub.client.success = function(msg, title) {
        return toastr.success(msg, title);
      };
      this.hub.client.info = function(msg, title) {
        return toastr.info(msg, title);
      };
      this.hub.client.warning = function(msg, title) {
        return toastr.warning(msg, title);
      };
      this.hub.client.error = function(msg, title) {
        console.error(msg);
        return toastr.error(msg, title);
      };
      $.connection.hub.start().fail(function(msg) {
        console.error(msg);
        return toastr.error(msg);
      });
    }

    Console.prototype.addMessage = function(message, className) {
      var msgbox;
      this.output.append($('<div/>').addClass(className).text(message));
      this.output.scrollTop(this.output.prop('scrollHeight'));
      msgbox = this.dialog.messageBox(typeof title !== "undefined" && title !== null ? title : "Success", message, [
        {
          label: 'Yes, I\'m sure',
          result: 'yes'
        }, {
          label: 'Nope',
          result: 'no'
        }
      ]);
      msgbox.open().then(function(result) {
        return console.log(result);
      });
      return this.scope.$apply();
    };

    Console.prototype.sendMessage = function(e) {
      var message;
      e.preventDefault();
      message = this.textBox.val();
      if (message.length === 0) {
        return;
      }
      try {
        this.hub.server.send(message);
      } catch (err) {
        toastr.error(err.message);
      }
      this.textBox.val('');
      return this.textBox.focus();
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
      controller: ['$scope', '$element', '$dialog', Console],
      link: function(scope, element, attrs) {}
    };
  });

}).call(this);

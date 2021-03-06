﻿(function() {
  'use strict';

  var Console, app,
    __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

  Console = (function() {

    function Console($scope, $element, $dialog) {
      this.sendMessage = __bind(this.sendMessage, this);

      this.login = __bind(this.login, this);

      this.removeUser = __bind(this.removeUser, this);

      this.addUser = __bind(this.addUser, this);

      this.setUsers = __bind(this.setUsers, this);

      this.addMessage = __bind(this.addMessage, this);
      this.scope = $scope;
      this.scope.messages = [];
      this.scope.users = [];
      this.el = $element;
      this.dialog = $dialog;
      this.outputScroll = this.el.find('.console > div');
      this.textBox = this.el.find('input[type="text"]');
      this.el.find('form').submit(this.sendMessage);
      this.el.find('input[type="button"]').click(this.sendMessage);
      this.hub = $.connection.clunchHub;
      this.hub.client.addMessage = this.addMessage;
      this.hub.client.setUsers = this.setUsers;
      this.hub.client.addUser = this.addUser;
      this.hub.client.removeUser = this.removeUser;
      this.hub.client.login = this.login;
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
      $scope.$on('$destroy', function() {
        return $.connection.hub.stop();
      });
    }

    Console.prototype.addMessage = function(name, text, className) {
      this.scope.messages.push({
        user: name,
        text: text,
        className: className
      });
      if (this.scope.messages.length > 300) {
        this.scope.messages.shift();
      }
      this.scope.$apply();
      return this.outputScroll.scrollTop(this.outputScroll.prop('scrollHeight'));
    };

    Console.prototype.setUsers = function(users) {
      this.scope.users = users;
      return this.scope.$apply();
    };

    Console.prototype.addUser = function(user) {
      toastr.info("User '" + user.Name + "' has connected.");
      this.scope.users.push(user);
      this.scope.users = _.sortBy(this.scope.users, 'Name');
      return this.scope.$apply();
    };

    Console.prototype.removeUser = function(user) {
      var u;
      toastr.warning("User '" + user.Name + "' has disconnected.");
      this.scope.users = (function() {
        var _i, _len, _ref, _results;
        _ref = this.scope.users;
        _results = [];
        for (_i = 0, _len = _ref.length; _i < _len; _i++) {
          u = _ref[_i];
          if (u.ConnectionId !== user.ConnectionId) {
            _results.push(u);
          }
        }
        return _results;
      }).call(this);
      return this.scope.$apply();
    };

    Console.prototype.login = function() {
      var dlg,
        _this = this;
      dlg = this.dialog.dialog({
        dialogClass: 'modal login',
        backdropClick: false,
        keyboard: false,
        template: '<div class="modal-header"><h4>Who are you?</h4></div>\n<div class="modal-body">\n    <form name="loginForm">\n        <input type="text" name="loginName" ng-model="name" required="required" size="30" maxlength="30" />\n    </form>\n</div>\n<div class="modal-footer">\n    <input type="button" class="btn" ng-click="login()" ng-disabled="loginForm.$invalid" value="Sign In" />\n</div>',
        controller: [
          '$scope', 'dialog', function($scope, dialog) {
            dialog.modalEl.find('form').submit(function() {
              return dialog.close($scope.name);
            });
            $scope.login = function() {
              return dialog.close($scope.name);
            };
            return _.delay(function() {
              return dialog.modalEl.find('input[name="loginName"]').focus();
            }, 250);
          }
        ]
      });
      dlg.open().then(function(name) {
        _this.hub.server.login(name);
        return _this.textBox.focus();
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
        console.error(err);
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
      templateUrl: 'Templates/console.html',
      controller: ['$scope', '$element', '$dialog', Console],
      link: function(scope, element, attrs) {}
    };
  });

}).call(this);

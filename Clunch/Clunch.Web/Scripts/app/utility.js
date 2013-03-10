(function() {
  var _base, _ref;

  (function(mod, $) {
    mod.buildTemplateUrl = function(templateFileName) {
      return '/Templates/' + templateFileName;
    };
    mod.renderTemplate = function(templateFileName, $el, data) {
      if (data == null) {
        data = {};
      }
      return mod.getHtmlFromTemplate(templateFileName, function(html) {
        return $el.empty().append(_.template(html, data));
      });
    };
    mod.getHtmlFromTemplate = function(templateFileName, callback) {
      return $.get(mod.buildTemplateUrl(templateFileName), function(template) {
        return callback(template);
      });
    };
    mod.serializeObject = function(selector) {
      var item, result, serializedArray, _i, _len, _ref1, _ref2;
      result = {};
      serializedArray = $(selector).serializeArray({
        checkboxesAsBools: true
      });
      for (_i = 0, _len = serializedArray.length; _i < _len; _i++) {
        item = serializedArray[_i];
        if (result[item.name] != null) {
          if (!(result[item.name].push != null)) {
            result[item.name] = [result[item.name]];
          }
          result[item.name].push((_ref1 = item.value) != null ? _ref1 : '');
        } else {
          result[item.name] = (_ref2 = item.value) != null ? _ref2 : '';
        }
      }
      return result;
    };
    mod.getBaseUrl = function() {
      var baseUrl, pathArray;
      pathArray = window.location.href.split('/');
      baseUrl = pathArray[0] + '//' + pathArray[1] + pathArray[2];
      if (baseUrl.substr(baseUrl.length - 1) !== '/') {
        baseUrl += '/';
      }
      return baseUrl;
    };
  })((_ref = (_base = window.Clunch).utility) != null ? _ref : _base.utility = {}, jQuery);

}).call(this);

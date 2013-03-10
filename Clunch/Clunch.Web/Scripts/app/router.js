(function() {

  (function($) {
    Clunch.App = function(contactsViewModel) {
      return $.sammy('#content', function() {
        var self,
          _this = this;
        self = this;
        this.contactViewModel = contactsViewModel;
        this.renderTemplate = function(html) {
          _this.$element().html(html);
          return ko.applyBindings(_this.contactViewModel);
        };
        this.get('#/', function() {
          return this.render('/Templates/contactDetail.htm', {}, function(html) {
            return self.renderTemplate(html);
          });
        });
        return this.get('#/create', function() {
          return this.render('/Templates/contactCreate.htm', {}, function(html) {
            return self.renderTemplate(html);
          });
        });
      });
    };
    return $(function() {
      return $.getJSON('/api/contacts', function(data) {
        var viewModel;
        viewModel = new Clunch.ViewModels.ContactsViewModel(data);
        return Clunch.App(viewModel).run('#/');
      });
    });
  })(jQuery);

}).call(this);

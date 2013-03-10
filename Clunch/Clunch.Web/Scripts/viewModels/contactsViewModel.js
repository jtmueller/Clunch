(function() {
  var _ref;

  (function(viewModel, $) {
    return viewModel.ContactsViewModel = function(contacts) {
      this.contacts = ko.observableArray(contacts);
      this.addContact = function() {
        var data,
          _this = this;
        data = Clunch.utility.serializeObject($('#contactForm'));
        return $.ajax({
          url: '/api/Contacts',
          data: JSON.stringify(data),
          type: 'POST',
          dataType: 'json',
          contentType: 'application/json'
        }).done(function() {
          toastr.success('You have successfully created a new contact!', 'Success!');
          _this.contacts.push(data);
          return window.location.href = '#/';
        }).fail(function() {
          return toastr.error('There was an error creating your new contact.', '<sad face>');
        });
      };
    };
  })((_ref = Clunch.ViewModels) != null ? _ref : Clunch.ViewModels = {}, jQuery);

}).call(this);

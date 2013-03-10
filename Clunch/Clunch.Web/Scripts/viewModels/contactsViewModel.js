(function() {
  var _ref;

  (function(viewModel, $) {
    return viewModel.ContactsViewModel = function(contacts) {
      this.contacts = ko.observableArray(contacts);
      this.addContact = function() {
        var data,
          _this = this;
        data = appFsMvc.utility.serializeObject($('#contactForm'));
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
  })((_ref = appFsMvc.ViewModels) != null ? _ref : appFsMvc.ViewModels = {}, jQuery);

}).call(this);

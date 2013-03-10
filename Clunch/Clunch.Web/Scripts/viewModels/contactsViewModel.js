(function() {
  var _ref;

  (function(viewModel, $) {
    return viewModel.ContactsViewModel = function(contacts) {
      var _this = this;
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
      this.getContact = function(id) {
        id = 'contacts/' + id;
        return _this.activeContact = _.find(_this.contacts(), function(contact) {
          return contact.Id === id;
        });
      };
      this.updateContact = function() {
        var _this = this;
        return $.ajax({
          url: '/api/Contacts',
          data: JSON.stringify(this.activeContact),
          type: 'POST',
          dataType: 'json',
          contentType: 'application/json'
        }).done(function() {
          toastr.success('You have successfully updated your contact!', 'Success!');
          return window.location.href = '#/';
        }).fail(function() {
          console.log('Error: Update Contact', arguments);
          return toastr.error('There was an error editing your contact.', '<sad face>');
        });
      };
      this.deleteContact = function(contact) {
        _this.contacts.splice(_this.contacts.indexOf(contact), 1);
        return $.ajax({
          url: '/api/Contacts',
          data: contact.Id,
          type: 'DELETE',
          contentType: 'application/json'
        }).done(function() {
          return toastr.success('You have successfully deleted your contact!', 'Success!');
        }).fail(function() {
          console.log('Error: Delete Contact', arguments);
          return toastr.error('There was an error editing your contact.', '<sad face>');
        });
      };
    };
  })((_ref = Clunch.ViewModels) != null ? _ref : Clunch.ViewModels = {}, jQuery);

}).call(this);

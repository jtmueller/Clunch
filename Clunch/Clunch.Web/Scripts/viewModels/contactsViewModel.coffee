# This is largely based on the example from http://knockoutjs.com/examples/contactsEditor.html
((viewModel, $) ->

    viewModel.ContactsViewModel = (contacts) ->
        @contacts = ko.observableArray contacts

        @addContact = ->
            data = Clunch.utility.serializeObject $('#contactForm')
            $.ajax(
                url: '/api/Contacts',
                data: JSON.stringify data
                type: 'POST'
                dataType: 'json'
                contentType: 'application/json'
            ).done((contact) =>
                toastr.success 'You have successfully created a new contact!', 'Success!'
                @contacts.push contact
                window.location.href = '#/'
            ).fail(->
                toastr.error 'There was an error creating your new contact.', '<sad face>'
            )

        @getContact = (id) =>
            id = 'contacts/' + id
            @activeContact =
                _.find @contacts(), (contact) ->
                    contact.Id == id

        @updateContact = ->
            $.ajax(
                url: '/api/Contacts',
                data: JSON.stringify @activeContact
                type: 'POST'
                dataType: 'json'
                contentType: 'application/json'
            ).done(=>
                toastr.success 'You have successfully updated your contact!', 'Success!'
                window.location.href = '#/'
            ).fail(->
                console.log 'Error: Update Contact', arguments
                toastr.error 'There was an error editing your contact.', '<sad face>'
            )

        @deleteContact = (contact) =>
            @contacts.splice @contacts.indexOf(contact), 1
            $.ajax(
                url: '/api/Contacts',
                data: contact.Id
                type: 'DELETE'
                contentType: 'application/json'
            ).done(->
                toastr.success 'You have successfully deleted your contact!', 'Success!'
            ).fail(->
                console.log 'Error: Delete Contact', arguments
                toastr.error 'There was an error editing your contact.', '<sad face>'
            )

        return
    
)(Clunch.ViewModels ?= {}, jQuery)

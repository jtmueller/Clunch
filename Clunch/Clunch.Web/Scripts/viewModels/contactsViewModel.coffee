# This is largely based on the example from http://knockoutjs.com/examples/contactsEditor.html
((viewModel, $) ->

    viewModel.ContactsViewModel = (contacts) ->
        @contacts = ko.observableArray contacts

        @addContact = ->
            data = appFsMvc.utility.serializeObject $('#contactForm')
            $.ajax(
                url: '/api/Contacts',
                data: JSON.stringify data
                type: 'POST'
                dataType: 'json'
                contentType: 'application/json'
            ).done(=>
                toastr.success 'You have successfully created a new contact!', 'Success!'
                @contacts.push data
                window.location.href = '#/'
            ).fail(->
                toastr.error 'There was an error creating your new contact.', '<sad face>'
            )

        return

)(appFsMvc.ViewModels ?= {}, jQuery)

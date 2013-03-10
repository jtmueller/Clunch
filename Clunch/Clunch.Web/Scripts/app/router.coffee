(($) ->
    Clunch.App = (contactsViewModel) ->

        $.sammy '#content', ->
            self = this
            @contactViewModel = contactsViewModel

            @renderTemplate = (html) =>
                @$element().html html
                ko.applyBindings @contactViewModel

            # display all contacts
            @get '#/', ->
                @render '/Templates/contactDetail.html', {}, (html) ->
                    self.renderTemplate html
            
            # display the create contacts view
            @get '#/create', ->
                @render '/Templates/contactCreate.html', {}, (html) ->
                    self.renderTemplate html

            # display the edit contact view
            @get '#/edit/contacts/:id', ->
                self.contactViewModel.getContact @params['id']
                @render '/Templates/contactEdit.html', {}, (html) ->
                    self.renderTemplate html

    $ ->
        $.getJSON '/api/contacts', (data) ->
            viewModel = new Clunch.ViewModels.ContactsViewModel data
            Clunch.App( viewModel ).run '#/'

)(jQuery)
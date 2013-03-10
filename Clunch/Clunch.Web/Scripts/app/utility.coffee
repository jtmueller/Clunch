((mod, $) ->

    mod.buildTemplateUrl = (templateFileName) ->
        '/Templates/' + templateFileName

    mod.renderTemplate = (templateFileName, $el, data) ->
        data ?= {}
        mod.getHtmlFromTemplate templateFileName, (html) ->
            $el.empty().append( _.template html, data )

    mod.getHtmlFromTemplate = (templateFileName, callback) ->
        $.get mod.buildTemplateUrl(templateFileName), (template) ->
            callback template

    mod.serializeObject = (selector) ->
        result = {}
        serializedArray = $(selector).serializeArray { checkboxesAsBools: true }
        for item in serializedArray
            if result[item.name]?
                if not result[item.name].push?
                    result[item.name] = [result[item.name]]
                result[item.name].push( item.value ? '' )
            else
                result[item.name] = item.value ? ''
        result

    mod.getBaseUrl = ->
        pathArray = window.location.href.split '/'
        baseUrl = pathArray[0] + '//' + pathArray[1] + pathArray[2]
        if baseUrl.substr(baseUrl.length - 1) != '/'
            baseUrl += '/'
        baseUrl

    return

)(window.Clunch.utility ?= {}, jQuery)

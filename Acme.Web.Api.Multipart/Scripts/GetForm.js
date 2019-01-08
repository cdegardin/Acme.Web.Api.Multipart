function getForm(model, replacer)
{
    var files = [];
    var result = new FormData();
    var json = JSON.stringify(model, function (k, v)
    {
        if (v instanceof FileList)
        {
            return Array.prototype.slice.call(v);
        }
        else if (v instanceof File || v instanceof Blob)
        {
            var file = files.indexOf(v);
            if (file === -1)
            {
                files.push(v);
                file = files.length - 1;
                result.append('$file$' + file + '$', v);
            }

            return '$file$' + file + '$';
        }
        else
        {
            return replacer ? replacer(k, v) : v;
        }
    });

    if (files.length)
    {
        result.append('$json$', new Blob([json], { type: 'application/json' }));
        return result;
    }
    else
    {
        return json;
    }
}
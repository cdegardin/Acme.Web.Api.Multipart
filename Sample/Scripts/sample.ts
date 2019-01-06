document.getElementById('sample-form').addEventListener('submit', async e =>
{
    e.preventDefault();
    const model =
    {
        email: (document.getElementById('email') as HTMLInputElement).value,
        password: (document.getElementById('password') as HTMLInputElement).value,
        photo: (document.getElementById('photo') as HTMLInputElement).files[0],
        contracts: (document.getElementById('contracts') as HTMLInputElement).files,
        check: (document.getElementById('check') as HTMLInputElement).checked
    };

    const submitModel = getForm(model);
    console.info('submit', model);
    var headers = submitModel instanceof FormData ? {} : { 'Content-Type': 'application/json' };
    var response = await fetch(
        '/api/register',
        {
            credentials: 'include',
            body: submitModel,
            method: 'post',
            headers: headers
        });
    console.info('Response', await response.json());
});

function getForm(model: any): string | FormData
{
    var files = 0;
    const result = new FormData();
    const json = JSON.stringify(model, (_, v) =>
    {
        if (v instanceof FileList)
        {
            return Array.prototype.slice.call(v);
        }
        else if (v instanceof File || v instanceof Blob)
        {
            var key = `$file$${++files}$`;
            result.append(key, v);
            return key;
        }
        else
        {
            return v;
        }
    });

    if (files > 0)
    {
        result.append('$json$', new Blob([json], { type: 'application/json' }));
        return result;
    }
    else
    {
        return json;
    }
}
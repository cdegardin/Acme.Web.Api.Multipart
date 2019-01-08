document.getElementById('sample-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    var duplicateFile = new Blob(['Super file!'], { type: 'text/plain' });
    const model = {
        email: document.getElementById('email').value,
        password: document.getElementById('password').value,
        photo: duplicateFile,
        contracts: document.getElementById('contracts').files,
        check: document.getElementById('check').checked,
        item: {
            id: new Date().getTime(),
            file: duplicateFile
        }
    };
    const submitModel = getForm(model);
    console.info('submit', model);
    var headers = submitModel instanceof FormData ? {} : { 'Content-Type': 'application/json' };
    var response = await fetch('/api/register', {
        credentials: 'include',
        body: submitModel,
        method: 'post',
        headers: headers
    });
    console.info('Response', await response.json());
});
function getForm(model, replacer) {
    const files = [];
    const result = new FormData();
    const json = JSON.stringify(model, (k, v) => {
        if (v instanceof FileList) {
            return Array.prototype.slice.call(v);
        }
        else if (v instanceof File || v instanceof Blob) {
            let file = files.indexOf(v);
            if (file === -1) {
                files.push(v);
                file = files.length - 1;
                result.append(`$file$${file}$`, v);
            }
            return `$file$${file}$`;
        }
        else {
            return replacer ? replacer(k, v) : v;
        }
    });
    if (files.length) {
        result.append('$json$', new Blob([json], { type: 'application/json' }));
        return result;
    }
    else {
        return json;
    }
}
//# sourceMappingURL=sample.js.map
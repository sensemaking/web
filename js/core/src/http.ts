export const get = (url : string) => api(Method.Get, url)
export const post = (url : string, body : object) => api(Method.Post, url, body)
export const put = (url : string, body : object) => api(Method.Put, url, body)
export const del = (url : string) => api(Method.Delete, url)

const mediaType = `application/json`

enum Method {
    Get = `GET`,
    Post = `POST`,
    Put = `PUT`,
    Delete = `DELETE`
}

function api(method : Method, url : string, body? : object) {
    return fetch(url, {
        method: method,
        headers:  body ? { 'Content-Type': mediaType, 'Accept': mediaType } : { 'Accept': mediaType },
        body: (body ? JSON.stringify(body) : null)
    }).then(jsonOrApiError)
}

function jsonOrApiError(response : Response) {
    return new Promise((resolve, reject) => {
        if (response.ok)
            response.json().then(body => resolve(body)).catch(_ => resolve(null));
        else if([401, 403].includes(response.status))
            reject({ status: response.status, message: `Access Denied`, url: response.url });
        else
            response.json()
                .then(body => { reject({ status: response.status, message: `Received API Problem`, problem: body, url: response.url })})
                .catch(_ => reject({ status: response.status, message: `Unable to deserialise response body`, url: response.url }));                     
    })
}    
export const get : any = (url : string) => api(Method.Get, url)
export const post : any = (url : string, payload : object) => api(Method.Post, url, payload)
export const put : any = (url : string, payload : object) => api(Method.Put, url, payload)
export const del : any = (url : string) => api(Method.Delete, url)

enum Method { Get = `GET`, Post = `POST`, Put = `PUT`, Delete = `DELETE` }
const mediaType = `application/json`

function api(method : Method, url : string, payload? : object) {
    return fetch(url, {
        method: method,
        headers:  payload ? { 'Content-Type': mediaType, 'Accept': mediaType } : { 'Accept': mediaType },
        body: (payload ? JSON.stringify(payload) : null)
    }).then(jsonOrApiError)
}

function jsonOrApiError(response : Response) {
    return new Promise((resolve, reject) => {
        if (response.ok)
            response.text().then(text => resolve(text !== "" ? JSON.parse(text) : null))
                        //    response.json().then(payload => resolve(payload)).catch(_ => resolve(null));
        else if([401, 403].includes(response.status))
            reject({ status: response.status, message: `Access Denied`, url: response.url });
        else
            response.json()
                .then(payload => { reject({ status: response.status, message: `Received API Problem`, problem: payload, url: response.url })})
                .catch(_ => reject({ status: response.status, message: `Unable to deserialise response body`, url: response.url }));                     
    })
}    
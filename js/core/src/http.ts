import HttpStatusCode from './http-status-codes'

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

async function jsonOrApiError(response : Response) {    
    const json = response.json().then(json => json).catch(() => null)
    
    if(response.ok)
        return json
    else      
        throw new Error(JSON.stringify({ status: response.status, message: [HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden].includes(response.status) ? `Access Denied` : `Received API Problem`, problem: json }))    
}

import HttpStatusCode from './http-status-codes'

export const get : any = (url: string) => api(Method.Get, url)
export const post : any = (url: string, payload : object) => api(Method.Post, url, payload)
export const put : any = (url: string, payload : object) => api(Method.Put, url, payload)
export const del : any = (url: string) => api(Method.Delete, url)

enum Method { Get = `GET`, Post = `POST`, Put = `PUT`, Delete = `DELETE` }
const mediaType = `application/json`

async function api(method: Method, url: string, payload?: object) {
    const response = await fetch(url, {
        method: method,
        headers:  payload ? { 'Content-Type': mediaType, 'Accept': mediaType } : { 'Accept': mediaType },
        body: (payload ? JSON.stringify(payload) : null)
    })
    
    return await jsonOrApiError(url, response)
}

async function jsonOrApiError(url : string, response : Response) {    
    const json = await response.json().then(json => json).catch(_ => null)
    
    if(response.ok)
        return json
    else      
        throw { status: response.status, statusText: response.statusText, url, problem: json }
}

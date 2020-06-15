import { HttpMethod } from './http'

export const get : any = (url: string) => call(HttpMethod.Get, url)
export const post : any = (url: string, payload : object) => call(HttpMethod.Post, url, payload)
export const put : any = (url: string, payload : object) => call(HttpMethod.Put, url, payload)
export const del : any = (url: string) => call(HttpMethod.Delete, url)

const mediaType = `application/json`

async function call(method: HttpMethod, url: string, payload?: object) {
    const response = await fetch(url, {
        method: method,
        headers:  payload ? { 'Content-Type': mediaType, 'Accept': mediaType } : { 'Accept': mediaType },
        body: (payload ? JSON.stringify(payload) : null)
    }
)
    return await jsonOrError(response, url, method, payload)
}

async function jsonOrError(response : Response, url : string, method: HttpMethod, payload? : object) {    
    const json = await response.json().then(json => json).catch(_ => null)
    
    if(response.ok)
        return json
    else      
        throw { request: { url, method, payload }, status: { code: response.status, text: response.statusText }, problem: json }
}
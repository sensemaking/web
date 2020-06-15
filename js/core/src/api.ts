import { HttpMethod } from './http'

export const get : any = (url: string) => call(HttpMethod.Get, url)
export const post : any = (url: string, payload : object) => call(HttpMethod.Post, url, payload)
export const put : any = (url: string, payload : object) => call(HttpMethod.Put, url, payload)
export const del : any = (url: string) => call(HttpMethod.Delete, url)

const mediaType = `application/json`

async function call(method: HttpMethod, url: string, payload?: object) {
    const request: RequestInit = {
        method: method,
        headers:  payload ? { 'Content-Type': mediaType, 'Accept': mediaType } : { 'Accept': mediaType },
        body: (payload ? JSON.stringify(payload) : null)
    }
    const response = await fetch(url, request)
    return await jsonOrError(url, request, response)
}

async function jsonOrError(url : string, request : RequestInit, response : Response) {    
    const json = await response.json().then(json => json).catch(_ => null)
    
    if(response.ok)
        return json
    else      
        throw { request: { url, method: request.method, payload: request.body }, status: { code: response.status, text: response.statusText }, problem: json }
}
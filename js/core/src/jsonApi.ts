import { HttpMethod, HttpStatus, createHttpStatus } from './http'

export async function get<T>(url: string) : Promise<T> { return call(HttpMethod.Get, url) }
export async function post(url: string, payload: object) { return call(HttpMethod.Post, url, payload) }
export async function put(url: string, payload: object) { return call(HttpMethod.Put, url, payload) }
export async function del(url: string) { return call(HttpMethod.Delete, url) }

async function call(method: HttpMethod, url: string, payload?: object) {
    const mediaType = `application/json`
    const response = await fetch(url, {
        method: method,
        headers: payload ? { 'Content-Type': mediaType, 'Accept': mediaType } : { 'Accept': mediaType },
        body: (payload ? JSON.stringify(payload) : null)
    })
    return await jsonOrError(response, url, method, payload)
}

async function jsonOrError(response: Response, url: string, method: HttpMethod, payload?: object) {
    const json = await response.json().then(json => json).catch(_ => null)
    if (response.ok)
        return json
    else
        throw new ApiError({ url, method, payload }, createHttpStatus(response.status), json);
}

type ErroredRequest = { url: string, method: string, payload?: object }

export class ApiError extends Error {
    constructor(request: ErroredRequest, status: HttpStatus, problem: object | null, message? : string) {
        super(message)
        Object.setPrototypeOf(this, new.target.prototype);
        this.request = request
        this.status = status
        this.problem = problem
    }
    
    readonly request: ErroredRequest
    readonly status: HttpStatus
    readonly problem: object | null
}
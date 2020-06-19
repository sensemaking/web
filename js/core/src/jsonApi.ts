import { HttpMethod, HttpStatus, createHttpStatus } from './http'

type JsonPayload = string|number|boolean|Date|JsonObject|JsonPayload[]
type JsonObject = { [key: string]: string|number|boolean|Date|JsonObject|JsonPayload[] }

export async function get<T>(url: string) : Promise<T> { return call(HttpMethod.Get, url) }
export async function post(url: string, payload: JsonPayload) { call(HttpMethod.Post, url, payload) }
export async function put(url: string, payload: JsonPayload) { call(HttpMethod.Put, url, payload) }
export async function del(url: string) { call(HttpMethod.Delete, url) }

function voids() : never {
    throw { error: `Wibble` }
}

async function call(method: HttpMethod, url: string, payload?: unknown) {
    const mediaType = `application/json`
    const response = await fetch(url, {
        method: method,
        headers: payload ? { 'Content-Type': mediaType, 'Accept': mediaType } : { 'Accept': mediaType },
        body: (payload ? JSON.stringify(payload) : null)
    })
    return await jsonOrError(response, url, method, payload)
}

async function jsonOrError(response: Response, url: string, method: HttpMethod, payload?: unknown) {
    const json = await response.json().then(json => json).catch(_ => null)
    if (response.ok)
        return json
    else
        throw new ApiError({ url, method, payload }, createHttpStatus(response.status), json);
}

type ErroredRequest = { url: string, method: string, payload?: unknown }

export class ApiError extends Error {
    constructor(request: ErroredRequest, status: HttpStatus, problem: unknown | null, message? : string) {
        super(message)
        Object.setPrototypeOf(this, new.target.prototype);
        this.request = request
        this.status = status
        this.problem = problem
    }
    
    readonly request: ErroredRequest
    readonly status: HttpStatus
    readonly problem: unknown | null
}
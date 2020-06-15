import { describe, test, expect, afterAll, afterEach, beforeAll } from '@jest/globals'
import 'whatwg-fetch'
import { setupServer } from 'msw/node'
import { rest as mock, response } from 'msw'
import { get, put, post, del } from './http'

const noHandler = async (req : any, res : any, ctx : any) => { 
    console.log(`No mocked handler setup for ${req.method} at ${req.url}`) 
    return res(ctx.status(404))
}

const server  = setupServer(mock.get(`*`, noHandler), mock.put(`*`, noHandler), mock.post(`*`, noHandler), mock.delete(`*`, noHandler))

beforeAll(() => server.listen())
afterEach(() => server.resetHandlers())
afterAll(() => server.close())

const url = `http://myapi.com/`
const success = { success: true }
const failure = { success: false }

const successfulWhen = (predicate : Function) => (req : any, res : any, ctx : any) => res(ctx.status(200), ctx.json(predicate(req) ? success : failure))
const hasBody = (payload : any) => (req : any) => req.body.wibble === payload.wibble
const isJsonHeader = (header : string) => (req : any) => req.headers.get(header) === `application/json` 

describe(`Methods`, () => {
    test(`GETs from a url`, async () => {        
        server.use(mock.get(url, async (req, res, ctx) => res(ctx.status(200), ctx.json(success))))

        return expect(await get(url)).toEqual(success)
    })

    test(`PUTs a body to a url`, async () => { 
        const payload = { wibble: `wobble` }
        server.use(mock.put(url, successfulWhen(hasBody(payload))))

        return expect(await put(url, payload)).toEqual(success)
    })

    test(`POSTs a body to a url`, async () => { 
        const payload = { wibble: `wobble` }
        server.use(mock.post(url, successfulWhen(hasBody(payload))))

        return expect(await post(url, payload)).toEqual(success)
    })

    test(`DELETEs from a url`, async () => { 
        server.use(mock.delete(url, async (req, res, ctx) => res(ctx.status(200), ctx.json(success))))        
        return expect(await del(url)).toEqual(success)
    })
})

describe(`Headers`, () => {        
    const checkForJson = (header : string) => successfulWhen(isJsonHeader(header))

    test(`All http requests add accept header for application/json`, async () => {        
        server.use(mock.get(url, checkForJson(`Accept`)), mock.put(url, checkForJson(`Accept`)), mock.post(url, checkForJson(`Accept`)), mock.delete(url, checkForJson(`Accept`)))

        expect(await get(url)).toEqual(success)
        expect(await put(url, {})).toEqual(success)
        expect(await post(url, {})).toEqual(success)
        return expect(await del(url)).toEqual(success)
    })

    test(`PUT & POST add content-type header for application/json`, async () => { 
        server.use(mock.put(url, checkForJson(`Content-Type`)), mock.post(url, checkForJson(`Content-Type`)))

        expect(await put(url, {})).toEqual(success)
        return expect(await post(url, {})).toEqual(success)
    })
})

describe(`Error Handling`, () => {})
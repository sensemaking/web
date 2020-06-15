import { describe, test, expect, afterAll, afterEach, beforeAll } from '@jest/globals'
import 'whatwg-fetch'
import { setupServer } from 'msw/node'
import { rest as mock } from 'msw'
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

describe(`Methods`, () => {
    test(`GETs from a url`, async () => {        
        const result = { wibble: `wobble` }
        server.use(mock.get(url, async (req, res, ctx) => res(ctx.status(200), ctx.json(result))))

        return expect(await get(url)).toEqual(result)
    })

    test(`PUTs a body to a url`, async () => { 
        const body = { wibble: `wobble` }
        server.use(mock.put(url, async (req, res, ctx) => { return res((req.body as any).wibble === body.wibble ? ctx.status(200) : ctx.status(500))}))

        await put(url, body)
    })

    test(`POSTs a body to a url`, async () => { 
        const body = { wibble: `wobble` }
        server.use(mock.post(url, async (req, res, ctx) => { return res((req.body as any).wibble === body.wibble ? ctx.status(200) : ctx.status(500))}))

        await post(url, body)
    })

    test(`DELETEs from a url`, async () => { 
        server.use(mock.delete(url, async (req, res, ctx) => res(ctx.status(200))))
        await del(url)
    })
})

describe(`Headers`, () => {
    const checkForJson = (header : string) => (req : any, res : any, ctx : any) => res(req.headers.get(header) === `application/json` ? ctx.status(200) : ctx.status(500))

    test(`All http requests add accept header for application/json`, async () => {        
        server.use(mock.get(url, checkForJson(`Accept`)), mock.put(url, checkForJson(`Accept`)), mock.post(url, checkForJson(`Accept`)), mock.delete(url, checkForJson(`Accept`)))

        await get(url)
        await put(url, {})
        await post(url, {})
        await del(url)
    })

    test(`PUT & POST add content-type header for application/json`, async () => { 
        server.use(mock.put(url, checkForJson(`Content-Type`)), mock.post(url, checkForJson(`Content-Type`)))

        await put(url, {})
        await post(url, {})
    })
})

describe(`Error Handling`, () => {})
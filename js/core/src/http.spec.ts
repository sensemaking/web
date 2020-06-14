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
const jsonMediaType = `application/json`;

describe(`Metohds`, () => {
    test(`GETs from a url`, async () => {        
        const result = { wibble: `wobble` }
        server.use(
            mock.get(url, async (req, res, ctx) => res(ctx.status(200), ctx.json(result))))

        return expect(await get(url)).toEqual(result)
    })

    test(`PUTs a body to a url`, async () => { })
    test(`POSTs a body to a url`, async () => { })

    test(`DELETEs from a url`, async () => { 
        server.use(
            mock.delete(url, async (req, res, ctx) => res(ctx.status(200))))

        await del(url)
    })
})

describe(`Headers`, () => {
    test(`All http requests add accept header for application/json`, async () => {        
        server.use(
            mock.get(url, async (req, res, ctx) => res(req.headers.get(`Accept`) === jsonMediaType ? ctx.status(200) : ctx.status(500))),
            mock.put(url, async (req, res, ctx) => res(req.headers.get(`Accept`) === jsonMediaType ? ctx.status(200) : ctx.status(500))),
            mock.post(url, async (req, res, ctx) => res(req.headers.get(`Accept`) === jsonMediaType ? ctx.status(200) : ctx.status(500))),
            mock.delete(url, async (req, res, ctx) => res(req.headers.get(`Accept`) === jsonMediaType ? ctx.status(200) : ctx.status(500)))
        )

        await get(url)
        await put(url, {})
        await post(url, {})
        await del(url)
    })

    test(`PUT & POST add content-type header for application/json`, async () => { 
        server.use(
            mock.put(url, async (req, res, ctx) => res(req.headers.get(`Content-Type`) === jsonMediaType ? ctx.status(200) : ctx.status(500))),
            mock.post(url, async (req, res, ctx) => res(req.headers.get(`Content-Type`) === jsonMediaType ? ctx.status(200) : ctx.status(500))),
        )

        await put(url, {})
        await post(url, {})
    })
})

describe(`Error Handling`, () => {})
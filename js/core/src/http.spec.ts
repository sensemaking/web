import { describe, test, expect, afterAll, afterEach, beforeAll } from '@jest/globals'
import 'whatwg-fetch'
import { setupServer } from 'msw/node'
import { rest } from 'msw'
import * as http from './http'

const server  = setupServer(
    rest.get(`*`, async (req, res, ctx) => { 
        console.log(`No mocked handler setup for ${req.url}`) 
        return res(ctx.status(404))
    })
)

beforeAll(() => server.listen())
afterEach(() => server.resetHandlers())
afterAll(() => server.close())

const url = `http://myapi.com/`
const jsonMediaType = `application/json`;

describe(`All http requests`, () => {
    test(`Add an accept header for application/json`, async () => {        
        server.use(
            rest.get(`${url}`, async (req, res, ctx) => res(req.headers.get(`Accept`) === jsonMediaType ? ctx.status(200) : ctx.status(500))),
            rest.put(url, async (req, res, ctx) => res(req.headers.get(`Accept`) === jsonMediaType ? ctx.status(200) : ctx.status(500))),
            rest.post(url, async (req, res, ctx) => res(req.headers.get(`Accept`) === jsonMediaType ? ctx.status(200) : ctx.status(500))),
            rest.delete(url, async (req, res, ctx) => res(req.headers.get(`Accept`) === jsonMediaType ? ctx.status(200) : ctx.status(500)))
        )

        await http.get(url)
        await http.put(url, {})
        await http.post(url, {})
        await http.del(url)
    })
})

describe(`Put & Post`, () => {
    test(`Add a content-type header for application/json`, async () => { 
        server.use(
            rest.put(url, async (req, res, ctx) => res(req.headers.get(`Content-Type`) === jsonMediaType ? ctx.status(200) : ctx.status(500))),
            rest.post(url, async (req, res, ctx) => res(req.headers.get(`Content-Type`) === jsonMediaType ? ctx.status(200) : ctx.status(500))),
        )

        await http.put(url, {})
        await http.post(url, {})
    })
})

describe(`Get`, () => {
    test(`GETs from a url`, async () => {        
        const result = { wibble: `wobble` }
        server.use(
            rest.get(`${url}`, async (req, res, ctx) => res(ctx.status(200), ctx.json(result))))

        return expect(await http.get(url)).toEqual(result)
    })
})

describe(`Put`, () => {
    test(`PUTs a body at a url`, async () => { })
})

describe(`Post`, () => {})
describe(`Del`, () => {})
describe(`Error Handling`, () => {})
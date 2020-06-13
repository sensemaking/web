import { describe, test, expect, afterAll, afterEach, beforeAll } from '@jest/globals'
import 'whatwg-fetch'
import { setupServer } from 'msw/node'
import { rest } from 'msw'
import * as http from './http'

const jsonMediaType = `application/json`;
const server  = setupServer(
    rest.get(`*`, async (req, res, ctx) => { 
        console.log(`No mocked handler setup for ${req.url}`) 
        return res(ctx.status(404))
    })
)

beforeAll(() => server.listen())
afterEach(() => server.resetHandlers())
afterAll(() => server.close())

describe(`Get`, () => {
    const url = `http://myapi.com/get`
    test(`GETs from a url`, async () => {        
        const result = { wibble: `wobble` }
        server.use(
            rest.get(`${url}`, async (req, res, ctx) => res(ctx.status(200), ctx.json(result))))

        return expect(await http.get(url)).toEqual(result)
    })

    test(`Adds an accept header for application/json`, async () => {        
        server.use(
            rest.get(`${url}`, async (req, res, ctx) => res(req.headers.get(`Accept`) === jsonMediaType ? ctx.status(200) : ctx.status(500))))

        await http.get(url)
    })
})

describe(`Put`, () => {
    test(`PUTs a body at a url`, async () => { })
    test(`Adds a content-type header for application/json`, async () => { })
    test(`Adds an accept header for application/json`, async () => { })
})

describe(`Post`, () => {})
describe(`Del`, () => {})
describe(`Error Handling`, () => {})
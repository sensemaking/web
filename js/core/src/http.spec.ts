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

describe(`Get`, () => {
    test(`Carries out an http GET request against a url`, async () => {        
        const url = `http://myapi.com/get`
        const result = { wibble: `wobble` }
        server.use(
            rest.get(`${url}`, async (req, res, ctx) => res(ctx.status(200), ctx.json(result)))
        )

        return expect(await http.get(url)).toEqual(result)
    })

    test(`Adds an accept header for application/json`, async () => {        
        const url = `http://myapi.com/get`
        const result = { wibble: `wobble` }
        server.use(
            rest.get(`${url}`, async (req, res, ctx) => { return req.headers.get(`Accept`) === `application/json` ? res(ctx.status(200)) : res(ctx.status(500)) })
        )

        await http.get(url)
    })
})

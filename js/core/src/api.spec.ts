import { describe, test, expect, beforeAll, afterAll, afterEach } from '@jest/globals'
import 'whatwg-fetch'
import { setupServer } from 'msw/node'
import { rest as mock, response } from 'msw'
import { get, put, post, del } from './api'
import { HttpMethod, HttpStatusCode } from './http'

const noHandler = async (req: any, res: any, ctx: any) => {
    console.log(`No mocked handler setup for ${req.method} at ${req.url}`)
    return res(ctx.status(404))
}

const server = setupServer(mock.get(`*`, noHandler), mock.put(`*`, noHandler), mock.post(`*`, noHandler), mock.delete(`*`, noHandler))

beforeAll(() => server.listen())
afterEach(() => server.resetHandlers())
afterAll(() => server.close())

const url = `http://myapi.com/`
const success = { success: true }
const failure = { success: false }

interface Payload { wibble: string }

const successfulWhen = (predicate: Function) => (req: any, res: any, ctx: any) => res(ctx.status(200), ctx.json(predicate(req) ? success : failure))
const hasPayload = (payload: Payload) => (req: any) => req.body.wibble === payload.wibble
const isJson = (header: string) => (req: any) => req.headers.get(header) === `application/json`

describe(`Methods`, () => {
    test(`GETs from a url`, async () => {
        server.use(mock.get(url, successfulWhen(() => true)))
        return expect(await get(url)).toStrictEqual(success)
    })

    test(`PUTs a body to a url`, async () => {
        const payload = { wibble: `wobble` }
        server.use(mock.put(url, successfulWhen(hasPayload(payload))))
        return expect(await put(url, payload)).toStrictEqual(success)
    })

    test(`POSTs a body to a url`, async () => {
        const payload = { wibble: `wobble` }
        server.use(mock.post(url, successfulWhen(hasPayload(payload))))
        return expect(await post(url, payload)).toStrictEqual(success)
    })

    test(`DELETEs from a url`, async () => {
        server.use(mock.delete(url, successfulWhen(() => true)))
        return expect(await del(url)).toStrictEqual(success)
    })

    test(`Returns null when response has no content`, async () => {
        server.use(mock.get(url, (_, res, ctx) => res(ctx.status(200))))
        return expect(await get(url)).toBeNull()
    })
})

describe(`Headers`, () => {
    test(`All http requests add accept header for application/json`, async () => {
        server.use(
            mock.get(url, successfulWhen(isJson(`Accept`))),
            mock.put(url, successfulWhen(isJson(`Accept`))),
            mock.post(url, successfulWhen(isJson(`Accept`))),
            mock.delete(url, successfulWhen(isJson(`Accept`)))
        )

        expect(await get(url)).toStrictEqual(success)
        expect(await put(url, {})).toStrictEqual(success)
        expect(await post(url, {})).toStrictEqual(success)
        return expect(await del(url)).toStrictEqual(success)
    })

    test(`PUT & POST add content-type header for application/json`, async () => {
        server.use(
            mock.put(url, successfulWhen(isJson(`Content-Type`))),
            mock.post(url, successfulWhen(isJson(`Content-Type`)))
        )

        expect(await put(url, {})).toStrictEqual(success)
        return expect(await post(url, {})).toStrictEqual(success)
    })
})

describe(`Error Handling`, () => {
    test(`Errors provide the requested url, method and payload, the http status code and its text, and any problems detailed in the http response`, async () => {
        const payload = { wibble: `wobble` }
        const status = HttpStatusCode.Forbidden
        const problem = { title: `Things ain't so good`, errors: [`What hasn't gone wrong`, `Catastophic rip in the space time continuum`] }

        server.use(mock.put(url, (_, res, ctx) => res(ctx.status(status), ctx.json(problem))))

        return put(url, payload).catch((apiError: any) => {
            expect(apiError.request.url).toBe(url)
            expect(apiError.request.method).toBe(HttpMethod.Put)
            expect(apiError.request.payload).toBe(JSON.stringify(payload))
            expect(apiError.status.code).toBe(status)
            expect(apiError.status.text).toBe(HttpStatusCode[status])
            return expect(apiError.problem).toStrictEqual(problem)
        })
    })
})
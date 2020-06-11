import React from 'react'
import { render } from "@testing-library/react"

import { ErrorHandler } from './error'
import { alert } from './logging'

jest.mock('./logging');

const OurError = (error) => <h1>{error}</h1>
const ErrorInRender = () => { i++; return <span>Nothing here</span> }

describe.skip('Error Handling', () => {
    test('logs errors in components', () => {
        const { getByText } = render(<ErrorHandler component={OurError}><ErrorInRender /></ErrorHandler>)
        expect(getByText("i is not defined")).toBeInTheDocument();  
        
        expect(alert.mock.calls[0][0]).toBe("0098");
        expect(alert.mock.calls[0][1]).toBe("i is not defined");
        expect(alert.mock.calls[0][2].message).toBe("i is not defined");
        expect(alert.mock.calls[0][3].componentStack).toContain("ErrorInRender");
        expect(alert.mock.calls[0][3].componentStack).toContain("ErrorHandler");
    });
        
    test('renders children when no error',() => {
        const { getByText } = render(<ErrorHandler component={OurError}><span>The real content</span></ErrorHandler>)
        expect(getByText("The real content")).toBeInTheDocument();
    });
});
import React from 'react'

import { alert } from './logging'

export class ErrorHandler extends React.Component { 
    constructor(props) { 
        super(props);
        this.state = { hasError: false, message: '' };        
    }

    static getDerivedStateFromError(error) {
        return { hasError: true, message: error.message };
    }

    componentDidMount() {
        window.addEventListener("unhandledrejection", this.promiseRejectionUnhandled.bind(this));
        window.addEventListener("error", this.errorUnhandled.bind(this));
    }

    componentDidCatch(error, errorInfo) {
        alert("0098", error.message, { message: error.message, stack: error.stack }, errorInfo);
    }
    
    promiseRejectionUnhandled(event) {
        if(!isServerError(event))
            alert(event?.reason?.code || "0098", `Unhandled Promise Rejection`, event.reason);
            
        this.setState({hasError: true, message: event.reason});
    }
   
    errorUnhandled(event) { 
        alert("0098",event.message, { message: event.message, stack: event.error.stack })               
        this.setState({hasError: true, message: event.message });
    }

    render() {
        return <>
            {!this.state.hasError ? this.props.children : this.props.component(this.state.message) }
        </>
    }
}

function isServerError(event) {
    return event?.reason?.status === 500;
}
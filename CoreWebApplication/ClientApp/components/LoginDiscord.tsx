// A '.tsx' file enables JSX support in the TypeScript compiler, 
// for more information see the following page on the TypeScript wiki:
// https://github.com/Microsoft/TypeScript/wiki/JSX
import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface CounterState {
    currentCount: number;
}

export class LoginDiscord extends React.Component<RouteComponentProps<{}>, CounterState> {
    constructor() {
        super();
        this.state = { currentCount: 0 };
    }

    public render() {
        return <div>
            <h1>Counter</h1>

            <p>This is a simple example of a React component.</p>

            <p>Current count: <strong>{this.state.currentCount}</strong></p>

            <button onClick={() => { this.incrementCounter() }}>Increment</button>
        </div>;
    }

    incrementCounter() {
        this.setState({
            currentCount: this.state.currentCount + 1
        });
    }
}
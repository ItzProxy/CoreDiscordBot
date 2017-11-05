// A '.tsx' file enables JSX support in the TypeScript compiler, 
// for more information see the following page on the TypeScript wiki:
// https://github.com/Microsoft/TypeScript/wiki/JSX
import * as React from 'react';
import { RouteComponentProps } from 'react-router';


export class LoginDiscord extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        const isLoggedIn = this.state.IsLoggedIn;
        return <div>
            <Button
                onPress={()=>}
                title="Learn More"
                color="#841584"
                accessibilityLabel="Learn more about this purple button"
            />
        </div>;
    }
}
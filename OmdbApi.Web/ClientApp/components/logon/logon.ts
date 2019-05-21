import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import axios from 'axios';
import { APIService } from '../../APIService';
const apiService = new APIService();

interface userlogin {
    username: string;
    password: string;
}

@Component
export default class LogonComponent extends Vue {
    // token will get from login method
    userlogindto: userlogin = <userlogin>{
        username: "",
        password: ""
    };
    loading: boolean = false;
    errorstate: boolean = false;
    message: string = "";
    $router: any;

    Logon() {
        debugger;
        let params = {
            username: this.userlogindto.username,
            password: this.userlogindto.password
        }

        //axios.defaults.baseURL = apiService.API_URL;
        //axios.defaults.headers.get['Accepts'] = 'application/json';
        //axios.defaults.headers.common['Access-Control-Allow-Origin'] = '*';
        //axios.defaults.headers.common['Access-Control-Allow-Headers'] = 'Origin, X-Requested-With, Content-Type, Accept';
        //axios.defaults.withCredentials = true;
        //axios.defaults.headers.common = {
        //    'X-Requested-With': 'XMLHttpRequest'
        //}

        axios({
            method: 'post',
            url: apiService.API_URL + '/api/user/authenticate',
            headers: {
                'Access-Control-Allow-Origin': '*',
                'Content-Type': 'application/json',
                'Access-Control-Allow-Methods': 'GET, PUT, POST, DELETE, OPTIONS',
                'Access-Control-Allow-Headers': '*'
            },
            withCredentials: true,
            data: params
        })

        .then((response: any) => {
            if (response.data.status == true) {
                localStorage.setItem('token', response.data.response);
                this.loading = false;
                this.errorstate = false;
                this.$router.push('movies');
            }
            else {
                this.loading = false;
                this.errorstate = true;
                this.message = response.data.response;
            }
        })
        .catch((error: any) => {
            console.log(error.message);
        });
    }
}
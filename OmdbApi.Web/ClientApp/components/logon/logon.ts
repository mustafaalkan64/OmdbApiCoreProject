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

    async Logon() {

        let params = {
            username: this.userlogindto.username,
            password: this.userlogindto.password
        }

        await axios.post(apiService.API_URL + '/api/user/authenticate', params)
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
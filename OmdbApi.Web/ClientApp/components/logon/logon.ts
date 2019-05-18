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
    message: string = "";

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
                this.$router.push('movies');
            }
            else {
                this.loading = false;
                this.message = response.data.response;
                alert(this.message);
            }
        })
        .catch((error: any) => {
            console.log(error.message);
        });
    }
}
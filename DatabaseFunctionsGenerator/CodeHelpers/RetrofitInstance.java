package !--packageName--!;

import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class RetrofitInstance {
    public  static Retrofit GetRetrofitInstance()
    {
        Retrofit retrofit;
        Retrofit.Builder builder;

        builder = new Retrofit.Builder()
                .baseUrl("!--url--!");
                .addConverterFactory(GsonConverterFactory.create());

        retrofit = builder.build();

        return retrofit;
    }
}
